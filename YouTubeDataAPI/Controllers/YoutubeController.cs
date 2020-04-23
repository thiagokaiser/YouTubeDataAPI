using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using YouTubeDataAPI.Models;
using Core.Models;
using Core.Services;
using Api.Apis;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace YouTubeDataAPI.Controllers
{
    public class YoutubeController : Controller
    {
        private readonly ILogger<YoutubeController> _logger;        
        private readonly YoutubeService service;
        private readonly YoutubeApi youtubeApi;
        private readonly PlaylistService playlistService;

        public YoutubeController(ILogger<YoutubeController> logger,                                 
                                 YoutubeApi youtubeApi,
                                 Core.Services.YoutubeService service,
                                 PlaylistService playlistService)
        {
            _logger = logger;
            this.youtubeApi = youtubeApi;
            this.service = service;
            this.playlistService = playlistService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("Index.Get");

            IEnumerable<Core.Models.Video> videos = new List<Core.Models.Video>();
            
            return View(videos);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string searchText)
        {
            _logger.LogInformation("Index.Post");

            var videos = youtubeApi.Search(searchText);

            foreach (var item in videos)
            {
                await SaveVideo(item);
            }

            var playlists = await playlistService.PlaylistList();
            
            ViewBag.playlists = playlists.Select(a => new SelectListItem() { Value = a.Id.ToString(), Text = a.Title }).ToList();         
            

            ViewBag.searchText = searchText;

            return View(videos); 
        }
        
        public async Task SaveVideo(Core.Models.Video video)
        {
            if (await service.VideoById(video.Id) == null)
            {
                await service.VideoAdd(video);
            }
            else
            {
                await service.VideoUpdate(video);
            }
        }

        public async Task<IActionResult> VideoSaved()
        {
            IEnumerable<Core.Models.Video> videos = await service.VideoList();
            return View(videos);
        }

        public async Task<IActionResult> VideoDelAll()
        {
            await service.VideoDelAll();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddVideoToPlaylist(string videoId, string valorSelect)
        {
            await service.AddVideoToPlaylist(new Core.Models.ViewModels.VideoPlaylistViewModel() { VideoId = videoId, PlaylistId = Int32.Parse(valorSelect) });
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
