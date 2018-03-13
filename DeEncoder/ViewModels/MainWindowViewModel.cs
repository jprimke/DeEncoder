using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;

namespace DeEncoder.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            CodecItems = new ObservableCollection<string>(Enum.GetNames(typeof(Codecs)));
            SelectedCodec = Codecs.Base64;

            EncodeCommand = new AsyncCommand(Encode);
            DecodeCommand = new AsyncCommand(Decode, () => SelectedCodec == Codecs.Base64);
            CloseCommand = new AsyncCommand<ThemedWindow>(Close);
        }


        public IMessageBoxService MessageBoxService => GetService<IMessageBoxService>();

        public string InputText
        {
            get => GetProperty(() => InputText);
            set => SetProperty(() => InputText, value);
        }

        public string OutputText
        {
            get => GetProperty(() => OutputText);
            set => SetProperty(() => OutputText, value);
        }

        public ObservableCollection<string> CodecItems
        {
            get => GetProperty(() => CodecItems);
            set => SetProperty(() => CodecItems, value);
        }

        public Codecs SelectedCodec
        {
            get => GetProperty(() => SelectedCodec);
            set => SetProperty(() => SelectedCodec, value);
        }

        public AsyncCommand EncodeCommand { get; set; }

        public AsyncCommand DecodeCommand { get; set; }

        public AsyncCommand<ThemedWindow> CloseCommand { get; set; }

        private Task Close(ThemedWindow wnd)
        {
            wnd?.Close();
            return Task.CompletedTask;
        }

        private Task Decode()
        {
            try
            {
                var decoder = new EncoderDecoder(SelectedCodec);
                OutputText = decoder.Decode(InputText);
            }
            catch (Exception e)
            {
                MessageBoxService.ShowMessage(e.Message, null, MessageButton.OK, MessageIcon.Hand);
            }

            return Task.CompletedTask;
        }

        private Task Encode()
        {
            try
            {
                var encoder = new EncoderDecoder(SelectedCodec);
                OutputText = encoder.Encode(InputText);
            }
            catch (Exception e)
            {
                MessageBoxService.ShowMessage(e.Message, null, MessageButton.OK, MessageIcon.Hand);
            }

            return Task.CompletedTask;
        }
    }
}
