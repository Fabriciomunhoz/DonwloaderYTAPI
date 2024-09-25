using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using Orangotango = System.IO.File;

namespace API_Downloader.Controllers
{


    [Route("api/youtube")]
    [ApiController]
    [EnableCors]

    public class YoutubeDownloadController : ControllerBase
    {


        [HttpGet("baixarvideo")]
        public async Task<IActionResult> BaixarVideo([FromQuery]string videoID, [FromQuery]string Url)
        {
            try
            {

                var youtube = new YoutubeClient();
                var video = await youtube.Videos.GetAsync($"{Url}");
                var title = video.Title.Replace("|", "").Replace("?", "");
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(Url);
                byte[] conteudoVideo;
                var videoStreamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
                var videoStream = await youtube.Videos.Streams.GetAsync(videoStreamInfo);
                await youtube.Videos.Streams.DownloadAsync(videoStreamInfo, $@"Storage\{Guid.NewGuid()}.{videoStreamInfo.Container}");
                using (var memoryStream = new MemoryStream())
                {
                    videoStream.CopyTo(memoryStream);
                    conteudoVideo = memoryStream.ToArray();
                }
                return File(conteudoVideo, "video/mp4", $"{video.Title}.mp4"); 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            

        }

        [HttpGet("obtertitulo")]
        public async Task<IActionResult> ObterTitulo(string Url)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync($"{Url}");
            var title = video.Title.Replace("|", "").Replace("?", "");
            return Ok(title);
        }



        [HttpPost("removervideo")]
        public IActionResult RemoverVideo()
        {

            string pasta = @"Storage";
            if (Directory.Exists(pasta))
            {
                Console.WriteLine("A pasta existe!");
                try
                {
                    string[] arquivos = Directory.GetFiles(pasta);
                    foreach (string arquivo in arquivos)
                    {
                        Orangotango.Delete(arquivo);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else
            {

            }

            return Ok();
        }


    }
}
