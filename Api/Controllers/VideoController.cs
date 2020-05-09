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

        public VideoController(VideoService service)
        {                        
            this.service = service;            
        }
                
        [HttpGet]
        public async Task<IActionResult> Index(string searchText, string pageToken)
        {
            var videos = await service.Search(searchText, pageToken);            
            
            ViewBag.playlists = videos.Playlists.Select(a => new SelectListItem() { Value = a.Id.ToString(), Text = a.Title }).ToList();
            ViewBag.searchText = searchText;

            return View(videos); 
        }

        [HttpGet]
        public async Task<IActionResult> VideoDetail(string videoId)
        {
            var videos = await service.VideoDetailById(videoId);
            return View(videos);
        }

        public async Task<IActionResult> VideoSaved()
        {            
            var videos = await service.VideoList();            
            return View(videos);
        }

        public async Task<IActionResult> VideoDelAll()
        {
            await service.VideoDelAll();
            return RedirectToAction("VideoSaved");
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
            return RedirectToAction("PlaylistDetail", "Playlist", new { id = Int32.Parse(valorSelect) });            
        }        
    }
}
