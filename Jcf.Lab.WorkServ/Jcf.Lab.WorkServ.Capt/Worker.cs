using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing.Imaging;

namespace Jcf.Lab.WorkServ.Capt
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private FilterInfoCollection _videoDevices;
        private VideoCaptureDevice? _videoSource;
        private string _path = @"C:\Capturas";

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            Directory.CreateDirectory(_path);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                    if (_videoDevices.Count > 0)
                    {
                        _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);
                        _videoSource.NewFrame += VideoSource_NewFrame;
                        _videoSource.Start();

                        // Aguarde 3 segundos para garantir captura
                        await Task.Delay(3000, stoppingToken);

                        if (_videoSource.IsRunning)
                        {
                            _videoSource.SignalToStop();
                            _videoSource.WaitForStop();
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Nenhuma webcam foi encontrada.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao capturar da webcam");
                    File.AppendAllText(Path.Combine(_path, "erros.txt"), ex.ToString() + "\n");
                }

                // Aguarde 1 minuto para a próxima captura
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _logger.LogInformation("Serviço finalizado");
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                string filename = Path.Combine(_path, $"webcam_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");
                eventArgs.Frame.Save(filename, ImageFormat.Jpeg);
                _logger.LogInformation($"Imagem capturada: {filename}");

                // Captura só uma vez
                _videoSource?.SignalToStop();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar a imagem da webcam");
            }
        }
    }
}
