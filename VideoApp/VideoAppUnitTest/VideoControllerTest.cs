using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Video_Streaming.Controllers;
using Video_Streaming.DbEntity;
using Video_Streaming.Services.Interface;
using System.Web;
using System.Web.Http.Results;
using System.Net.Http;
using System.Web.Http;
using Castle.Core.Configuration;
using System.Configuration;
using System.IdentityModel;

namespace VideoAppUnitTest
{
    [TestClass]
    public class VideoControllerTest
    {

        private Mock<IVideoService> videoServiceMock;
        private VideoManagerController videoManagerController;

        [TestInitialize]
        public void Setup()
        {
            videoServiceMock = new Mock<IVideoService>();
            videoManagerController = new VideoManagerController(videoServiceMock.Object);
        }

        [TestMethod]
        public async Task GetAllVideoFiles_ShouldReturnOkWithVideoList()
        {
            List<VideoInformation> expectedVideoList = new List<VideoInformation>
                {
                    new VideoInformation
                    {
                        Title = "Test Title",
                        Description = "Test Description",
                        FileSize = 20,
                        VideoUrl = "C:/Projects/video.mp4",
                        Author = "Test_User"
                    }
                };

            videoServiceMock.Setup(x => x.GetAllVideoFiles()).Returns(expectedVideoList);

            // Act
            var result = await videoManagerController.GetAllVideoFiles();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<IEnumerable<VideoInformation>>));

            var contentResult = result as OkNegotiatedContentResult<IEnumerable<VideoInformation>>;
            CollectionAssert.AreEqual(expectedVideoList, contentResult.Content.ToList());
        }

        [TestMethod]
        public void ViewVideoFile_ShouldThrowException()
        {
            // Arrange
            int fileId = 6;

            // Act
            IHttpActionResult result = videoManagerController.ViewVideoFile(fileId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ExceptionResult));

        }

        [TestMethod]
        public async Task EditVideoFile_ShouldReturnOkWithResult()
        {
            // Arrange
            VideoInformation videoInformation = new VideoInformation()
            {
                Title = "Test Title",
                Description = "Test Description",
                Id = 5
            };

            videoServiceMock.Setup(x => x.UpdateVideoFile(It.IsAny<VideoInformation>())).Returns(1);

            // Act
            IHttpActionResult result = await videoManagerController.EditVideoFile(videoInformation);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<int>));

            var contentResult = result as OkNegotiatedContentResult<int>;
            Assert.AreEqual(1, contentResult.Content);
        }

        [TestMethod]
        public async Task EditVideoFile_ShouldReturnBadRequestResultWithErrorMessage()
        {
            // Arrange
            VideoInformation videoInformation = new VideoInformation()
            {
                Title = "Test Title",
                Description = "Test Description",
                Id = 5
            };

            videoServiceMock.Setup(x => x.UpdateVideoFile(It.IsAny<VideoInformation>())).Returns(-1);

            // Act
            IHttpActionResult result = await videoManagerController.EditVideoFile(videoInformation);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            var badRequestResult = result as BadRequestErrorMessageResult;
            Assert.AreEqual("Error occurred while updating video file data", badRequestResult.Message);
        }


        [TestMethod]
        public async Task DeleteVideoFile_ShouldReturnOkWithResult()
        {
            // Arrange
            int fileId = 6;
            videoServiceMock.Setup(x => x.DeleteVideoFile(It.IsAny<int>())).Returns(1);

            // Act
            IHttpActionResult result = await videoManagerController.DeleteVideoFile(fileId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<int>));

            var contentResult = result as OkNegotiatedContentResult<int>;
            Assert.AreEqual(1, contentResult.Content);
        }

        [TestMethod]
        public async Task DeleteVideoFile_ShouldReturnBadRequestResultWithErrorMessage()
        {
            // Arrange
            int fileId = 6;
            videoServiceMock.Setup(x => x.DeleteVideoFile(It.IsAny<int>())).Returns(-1);

            // Act
            IHttpActionResult result = await videoManagerController.DeleteVideoFile(fileId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestErrorMessageResult));

            var badRequestResult = result as BadRequestErrorMessageResult;
            Assert.AreEqual("Error occurred while deleting video file data", badRequestResult.Message);
        }


    }
}
