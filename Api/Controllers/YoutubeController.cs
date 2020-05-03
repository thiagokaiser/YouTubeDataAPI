using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Core.ViewModels;

namespace YouTubeDataAPI.Controllers
{
    public class YoutubeController : Controller
    {        
        private readonly YoutubeService service;        
        private readonly PlaylistService playlistService;

        public YoutubeController(Core.Services.YoutubeService service, PlaylistService playlistService)
        {                        
            this.service = service;
            this.playlistService = playlistService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Core.Models.Video> videos = new List<Core.Models.Video>();            
            return View(videos);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string searchText)
        {            
            var videos = service.Search(searchText);

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
            await service.AddVideoToPlaylist(new VideoPlaylistViewModel() { VideoId = videoId, PlaylistId = Int32.Parse(valorSelect) });
            return RedirectToAction("Index");
        }        
    }
}
