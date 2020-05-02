using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly PlaylistService service;

        public PlaylistController(PlaylistService service)
        {
            this.service = service;
        }
        public async Task<IActionResult> PlaylistDetail(int id)
        {
            var playlist = await service.PlaylistById(id);
            var videos = await service.PlaylistVideosById(id);
            playlist.Videos = videos.ToList();

            return View(playlist);
        }
        public async Task<IActionResult> Index()
        {
            var playlists = await service.PlaylistList();
            return View(playlists);
        }
        public async Task<IActionResult> VideoDel(int id)
        {
            await service.VideoDel(id);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> PlaylistDel(int id)
        {
            await service.PlaylistDel(id);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> PlaylistSave(Playlist playlist)
        {
            if (playlist.Id > 0)
            {
                await service.PlaylistUpdate(playlist);
            }
            else
            {
                await service.PlaylistAdd(playlist);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> PlaylistEdit(int id)
        {            
            var playlist = await service.PlaylistById(id) ?? new Playlist();
            return View(playlist);
        }        
    }
}
