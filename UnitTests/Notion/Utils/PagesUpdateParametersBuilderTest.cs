using ExportKindleClippingsToNotion.Notion.Utils;
using FakeItEasy;
using JetBrains.Annotations;
using Notion.Client;

namespace UnitTests.Notion.Utils;

[TestSubject(typeof(PagesUpdateParametersBuilder))]
public class PagesUpdateParametersBuilderTest
{

    [Fact]
        public void Build_ReturnsExpectedParameters()
        {
            var builder = new PagesUpdateParametersBuilder();
            builder.WithProperty("Name1", new TitlePropertyValue())
                   .WithProperty("Name2", new TitlePropertyValue())
                   .WithIcon(A.Fake<IPageIcon>())
                   .WithCover(A.Fake<FileObject>())
                   .WithIsArchived(true);
            
            var parameters = builder.Build();
            
            Assert.NotNull(parameters);
            Assert.Equal(2, parameters.Properties.Count);
        }

        [Fact]
        public void Reset_ResetsBuilderState()
        {
            var builder = new PagesUpdateParametersBuilder();
            builder.WithProperty("Name", new TitlePropertyValue())
                   .WithIcon(A.Fake<IPageIcon>())
                   .WithCover(A.Fake<FileObject>())
                   .WithIsArchived(true);
            
            builder.Reset();
            
            Assert.Empty(builder.Build().Properties);
            Assert.Null(builder.Build().Icon);
            Assert.Null(builder.Build().Cover);
            Assert.False(builder.Build().Archived);
        }

        [Fact]
        public void WithProperty_AddsPropertyToParameters()
        {
            var builder = new PagesUpdateParametersBuilder();
            
            builder.WithProperty("Name", new TitlePropertyValue());
            
            Assert.Single(builder.Build().Properties);
            Assert.Contains("Name", builder.Build().Properties.Keys);
            Assert.Equal("Notion.Client.TitlePropertyValue", builder.Build().Properties["Name"].ToString());
        }

        [Fact]
        public void WithIcon_SetsIconForParameters()
        {
            var builder = new PagesUpdateParametersBuilder();
            var fakeIcon = A.Fake<IPageIcon>();
            
            builder.WithIcon(fakeIcon);
            
            Assert.Equal(fakeIcon, builder.Build().Icon);
        }

        [Fact]
        public void WithCover_SetsCoverForParameters()
        {
            var builder = new PagesUpdateParametersBuilder();
            var fakeCover = A.Fake<FileObject>();
            
            builder.WithCover(fakeCover);
            
            Assert.Equal(fakeCover, builder.Build().Cover);
        }

        [Fact]
        public void WithIsArchived_SetsArchivedFlagForParameters()
        {
            var builder = new PagesUpdateParametersBuilder();
            
            builder.WithIsArchived(true);
            
            Assert.True(builder.Build().Archived);
        }
}