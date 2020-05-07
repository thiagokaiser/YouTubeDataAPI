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
    public class VideoController : Controller
    {        
        private readonly VideoService service;        
        private readonly PlaylistService playlistService;

        public VideoController(VideoService service, PlaylistService playlistService)
        {                        
            this.service = service;
            this.playlistService = playlistService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var videos = new List<Video>();            
            return View(videos);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string searchText)
        {            
            var videos = await service.Search(searchText);
            var playlists = await playlistService.PlaylistList();
            
            ViewBag.playlists = playlists.Select(a => new SelectListItem() { Value = a.Id.ToString(), Text = a.Title }).ToList();
            ViewBag.searchText = searchText;

            return View(videos); 
        }

        [HttpGet]
        public async Task<IActionResult> VideoDetail()
        {
            var videos = await service.VideoDetailById("UT66HeAptww");
            return Ok(videos);
        }

        public async Task<IActionResult> VideoSaved()
        {
            var playlists = await playlistService.PlaylistList();
            var videos = await service.VideoList();

            ViewBag.playlists = playlists.Select(a => new SelectListItem() { Value = a.Id.ToString(), Text = a.Title }).ToList();
            
            return View(videos);
        }

        public async Task<IActionResult> VideoDelAll()
        {
            await service.VideoDelAll();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddVideoToPlaylist(string videoId, string valorSelect)
        {
            try
            {
                await service.AddVideoToPlaylist(new VideoPlaylistViewModel() { VideoId = videoId, PlaylistId = Int32.Parse(valorSelect) });
            }
            catch (Exception ex)
            {                
                TempData["Message"] = ex.Message;
            }            
            return RedirectToAction("Index");
        }        
    }
}
