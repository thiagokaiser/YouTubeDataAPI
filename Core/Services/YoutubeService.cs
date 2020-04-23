using Core.Interfaces;
using Core.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class YoutubeService
    {
        private readonly IRepositoryYoutube repository;

        public YoutubeService(IRepositoryYoutube repository)
        {
            this.repository = repository;
        }

        public async Task VideoAdd(Core.Models.Video video)
        {
            await repository.VideoAdd(video);
        }

        public async Task VideoUpdate(Core.Models.Video video)
        {
            await repository.VideoUpdate(video);
        }

        public async Task<IEnumerable<Core.Models.Video>> VideoList()
        {
            return await repository.VideoList();
        }

        public async Task<Core.Models.Video> VideoById(string id)
        {
            return await repository.VideoById(id);
        }

        public async Task VideoDelAll()
        {
            await repository.VideoDelAll();
        }

        public async Task AddVideoToPlaylist(VideoPlaylistViewModel videoPlaylistViewModel)
        {
            await repository.AddVideoToPlaylist(videoPlaylistViewModel);
        }
    }
}
