namespace MyApp.MauiPages;

public partial class ScanQrCodePopup
{
    public ScanQrCodePopup()
	{
		InitializeComponent();
    }

	private async void scanner_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
	{
		var str = e.Results[0].Value;

		scanner.IsDetecting = false;

		await CloseAsync(str);
	}
}
