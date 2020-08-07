using Moq;
using Prism.Regions;
using System.Collections.Generic;
using UnityCommander.Business;
using UnityCommander.Modules.FilePanel.ViewModels;
using UnityCommander.Services.Interfaces;
using Xunit;

namespace UnityCommander.Modules.ModuleName.Tests.ViewModels
{
    public class ViewAViewModelFixture
    {
        Mock<IFilesProvider> _messageServiceMock;
        Mock<IRegionManager> _regionManagerMock;
        const List<FileModel> MessageServiceDefaultMessage = null;

        public ViewAViewModelFixture()
        {
            var messageService = new Mock<IFilesProvider>();
            messageService.Setup(x => x.GetFiles()).Returns(MessageServiceDefaultMessage);
            _messageServiceMock = messageService;

            _regionManagerMock = new Mock<IRegionManager>();
        }

        [Fact]
        public void MessagePropertyValueUpdated()
        {
            var vm = new ViewAViewModel(_regionManagerMock.Object, _messageServiceMock.Object);

            _messageServiceMock.Verify(x => x.GetFiles(), Times.Once);

            Assert.Equal(MessageServiceDefaultMessage, vm.FileList);
        }

        [Fact]
        public void MessageINotifyPropertyChangedCalled()
        {
            var vm = new ViewAViewModel(_regionManagerMock.Object, _messageServiceMock.Object);
            Assert.PropertyChanged(vm, nameof(vm.FileList), () => vm.FileList = new System.Collections.ObjectModel.ObservableCollection<FileModel>());
        }
    }
}
