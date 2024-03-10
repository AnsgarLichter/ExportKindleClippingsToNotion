using System.Collections;
using System.Globalization;
using ExportKindleClippingsToNotion.Parser;
using JetBrains.Annotations;

namespace UnitTests.Parser;

[TestSubject(typeof(ClippingsParserRussian))]
public class ClippingsParserRussianTest
{
    [Fact]
    public void ReturnsNullForAnInvalidClipping()
    {
        var parser = new ClippingsParserRussian();
        const string clipping = "line1\nline2\nline3";

        Assert.Null(parser.Parse(clipping));
    }

    [Theory]
    [ClassData(typeof(ValidRussianClippingTestData))]
    public void ReturnsAValidClipping(string clipping, string expectedAuthor, string expectedTitle,
        int expectedStartPosition, int expectedFinishPosition, int expectedPage, DateTime expectedHighlightDate,
        string expectedText)
    {
        var parser = new ClippingsParserRussian();
        var result = parser.Parse(clipping);

        Assert.NotNull(result);
        Assert.Equal(expectedAuthor, result.Author);
        Assert.Equal(expectedTitle, result.Title);
        Assert.Equal(expectedStartPosition, result.StartPosition);
        Assert.Equal(expectedFinishPosition, result.FinishPosition);
        Assert.Equal(expectedPage, result.Page);
        Assert.Equal(expectedHighlightDate, result.HighlightDate);
        Assert.Equal(expectedText, result.Text);
    }

    [Theory]
    [ClassData(typeof(InvalidRussianClippingTestData))]
    public void ReturnsBestMatchForInvalidClipping(string clipping, string expectedAuthor, string expectedTitle,
        int expectedStartPosition, int expectedFinishPosition, int expectedPage, DateTime expectedHighlightDate,
        string expectedText)
    {
        var parser = new ClippingsParserRussian();
        var result = parser.Parse(clipping);

        Assert.NotNull(result);
        Assert.Equal(expectedAuthor, result.Author);
        Assert.Equal(expectedTitle, result.Title);
        Assert.Equal(expectedStartPosition, result.StartPosition);
        Assert.Equal(expectedFinishPosition, result.FinishPosition);
        Assert.Equal(expectedPage, result.Page);
        Assert.Equal(expectedHighlightDate, result.HighlightDate);
        Assert.Equal(expectedText, result.Text);
    }

    [Fact]
    public void ReturnsNullIfLimitHasBeenReached()
    {
        const string clipping =
            "Врата Абаддона (Кори Джеймс)\n– Ваш выделенный отрывок в месте 442–442 | Добавлено: среда, 28 февраля 2024 г. в 11:30:54\n\n <Вы достигли максимального количества элементов буфера обмена для этого содержимого> ";
        var parser = new ClippingsParserEnglish();

        Assert.Null(parser.Parse(clipping));
    }
}

public class ValidRussianClippingTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            "Долгая прогулка (Стивен Кинг)\n- Ваша заметка в месте 5685 | Добавлено: среда, 28 февраля 2024 г. в 10:07:53\n\nГоворит Бейкер, один из идущих",
            "Стивен Кинг",
            "Долгая прогулка",
            5685,
            0,
            0,
            DateTime.Parse("2024-02-28T10:07:53"),
            "Говорит Бейкер, один из идущих"
        };

        yield return new object[]
        {
            "Долгая прогулка (Стивен Кинг)\n– Ваш выделенный отрывок в месте 5683–5685 | Добавлено: среда, 28 февраля 2024 г. в 10:07:53\n\nВспоминаю, как читал «Женщину в белом» Уилки Коллинза…",
            "Стивен Кинг",
            "Долгая прогулка",
            5683,
            5685,
            0,
            DateTime.Parse("2024-02-28T10:07:53"),
            "Вспоминаю, как читал «Женщину в белом» Уилки Коллинза…"
        };

        yield return new object[]
        {
            "Врата Абаддона (Кори Джеймс)\n– Ваш выделенный отрывок в месте 442–442 | Добавлено: среда, 28 февраля 2024 г. в 11:30:54\n\nКапитан",
            "Кори Джеймс",
            "Врата Абаддона",
            442,
            442,
            0,
            DateTime.Parse("2024-02-28T11:30:54"),
            "Капитан"
        };

        yield return new object[]
        {
            "Врата Абаддона (Кори Джеймс)\n– Ваш выделенный отрывок в месте 442–442 | Добавлено: среда, 28 февраля 2024 г. в 11:30:54\n\nКапитан",
            "Кори Джеймс",
            "Врата Абаддона",
            442,
            442,
            0,
            DateTime.Parse("2024-02-28T11:30:54"),
            "Капитан"
        };

        yield return new object[]
        {
            "Джонатан Стрендж и мистер Норрелл (Сюзанна Кларк)\n– Ваш выделенный отрывок в месте 142–142 | Добавлено: среда, 28 февраля 2024 г. в 15:21:17\n\nВальтера",
            "Сюзанна Кларк",
            "Джонатан Стрендж и мистер Норрелл",
            142,
            142,
            0,
            DateTime.Parse("2024-02-28T15:21:17"),
            "Вальтера"
        };
        yield return new object[]
        {
            "Джонатан Стрендж и мистер Норрелл (Сюзанна Кларк)\n– Ваша заметка в месте 142 | Добавлено: среда, 28 февраля 2024 г. в 15:21:46\n\nага",
            "Сюзанна Кларк",
            "Джонатан Стрендж и мистер Норрелл",
            142,
            0,
            0,
            DateTime.Parse("2024-02-28T15:21:46"),
            "ага"
        };
        yield return new object[]
       {
           "Врата Абаддона (Джеймс Кори)\n– Ваша заметка на странице 868 | Место 8411 | Добавлено: воскресенье, 3 марта 2024 г. в 18:08:16\n\nПроверка",
           "Джеймс Кори",
           "Врата Абаддона",
           8411,
           0,
           868,
           DateTime.Parse("2024-03-03T18:08:16"),
           "Проверка"
       };
        yield return new object[]
        {
            "Врата Абаддона (Джеймс Кори)\n– Ваш выделенный отрывок на странице 867 | Место 8408–8411 | Добавлено: воскресенье, 3 марта 2024 г. в 18:08:16\n\nДверь открылась, и он рывком сдвинул джойстик вперед. Мех провел его сквозь проем. Закрыв за собой, Бык, не медля и не раздумывая, свернул по коридору к внутреннему лифту, к долгому переходу на второй уровень, к сектору М.",
            "Джеймс Кори",
            "Врата Абаддона",
            8408,
            8411,
            867,
            DateTime.Parse("2024-03-03T18:08:16"),
            "Дверь открылась, и он рывком сдвинул джойстик вперед. Мех провел его сквозь проем. Закрыв за собой, Бык, не медля и не раздумывая, свернул по коридору к внутреннему лифту, к долгому переходу на второй уровень, к сектору М."
        };
        yield return new object[]
        {
            "К востоку от Эдема (Джон Стейнбек)\n– Ваша заметка на странице 6 | Место 55 | Добавлено: воскресенье, 3 марта 2024 г. в 18:48:36\n\nПроверка",
            "Джон Стейнбек",
            "К востоку от Эдема",
            55,
            0,
            6,
            DateTime.Parse("2024-03-03T18:48:36"),
            "Проверка"
        };
        yield return new object[]
        {
            "К востоку от Эдема (Джон Стейнбек)\n– Ваш выделенный отрывок на странице 6 | Место 53–55 | Добавлено: воскресенье, 3 марта 2024 г. в 18:48:36\n\nСалинас-Вэлли расположен в Северной Калифорнии и представляет собой длинную узкую полоску равнины между двумя цепями гор, посреди которой бежит, извиваясь",
            "Джон Стейнбек",
            "К востоку от Эдема",
            53,
            55,
            6,
            DateTime.Parse("2024-03-03T18:48:36"),
            "Салинас-Вэлли расположен в Северной Калифорнии и представляет собой длинную узкую полоску равнины между двумя цепями гор, посреди которой бежит, извиваясь"
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class InvalidRussianClippingTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            "Джеймс Кори (Врата Абаддона(2008))\n– Ваша заметка на странице 868 | Место 8411 | Добавлено: воскресенье, 3 марта 2024 г. в 18:08:16\n\nПроверка",
            "2008",
            "Джеймс Кори",
            8411,
            0,
            868,
            DateTime.Parse("2024-03-03T18:08:16"),
            "Проверка"
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}