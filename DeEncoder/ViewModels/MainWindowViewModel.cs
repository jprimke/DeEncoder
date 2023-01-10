using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using Microsoft.Extensions.Logging;

namespace DeEncoder.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ILogger<MainWindowViewModel> logger;

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger)
    {
        this.logger = logger;
        
        CodecItems = new ObservableCollection<string>(Enum.GetNames(typeof(Codecs)));
        SelectedCodec = Codecs.Base64;
        Salt = string.Empty;

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

    public string Salt
    {
        get => GetProperty(() => Salt);
        set => SetProperty(() => Salt, value);
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
            if (SelectedCodec != Codecs.Base64)
            {
                throw new UnreachableException("Selected codec is not supported");
            }
            
            logger.LogInformation("Decoding for text \"{InputText}\" with codec {SelectCodec}", InputText, SelectedCodec);

            var decoder = new EncoderDecoder(SelectedCodec);
            OutputText = decoder.Decode(InputText);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while decoding \"{InputText}\" with codec {SelectedCodec}: {Message}", InputText, SelectedCodec, e.Message);

            MessageBoxService.ShowMessage(
                                          e.Message,
                                          null,
                                          MessageButton.OK,
                                          MessageIcon.Hand
                                         );
        }

        return Task.CompletedTask;
    }

    private Task Encode()
    {
        try
        {
            logger.LogInformation("Encode for text \"{InputText}\" and salt \"{Salt}\" with codec {SelectedCodec}", InputText, Salt, SelectedCodec);
            
            var encoder = new EncoderDecoder(SelectedCodec);
            OutputText = encoder.Encode(InputText, Salt);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while encoding \"{InputText}\" with salt \"{Salt}\" and codec {SelectedCodec}: {Message}", InputText, Salt, SelectedCodec, e.Message);
            
            MessageBoxService.ShowMessage(
                                          e.Message,
                                          null,
                                          MessageButton.OK,
                                          MessageIcon.Hand
                                         );
        }

        return Task.CompletedTask;
    }
}
