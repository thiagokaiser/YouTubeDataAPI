using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Services
{
    public class PlaylistService
    {
        private readonly IRepositoryPlaylist repository;

        public PlaylistService(IRepositoryPlaylist repository)
        {
            this.repository = repository;
        }

        public async Task<Playlist> PlaylistById(int id)
        {
            var playlist = await repository.PlaylistById(id);
            return playlist;
        }

        public async Task<IEnumerable<Video>> PlaylistVideosById(int id)
        {
            var videos = await repository.PlaylistVideosById(id);
            return videos;
        }

        public async Task<IEnumerable<Playlist>> PlaylistList()
        {
            var playlists = await repository.PlaylistList();
            return playlists;
        }

        public async Task VideoDel(int id)
        {
            await repository.VideoDel(id);
        }

        public async Task PlaylistDel(int id)
        {
            await repository.PlaylistDel(id);
        }     
        
        public async Task PlaylistAdd(Playlist playlist)
        {
            await repository.PlaylistAdd(playlist);
        }

        public async Task PlaylistUpdate(Playlist playlist)
        {
            await repository.PlaylistUpdate(playlist);
        }
    }
}
