using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRepositoryVideo
    {
        Task VideoUpdate(Video video);
        Task VideoAdd(Video video);
        Task<Video> VideoById(string id);
        Task<Video> VideoDetailById(string id);
        Task<IEnumerable<Video>> VideoList();
        Task VideoDelAll();
        Task AddVideoToPlaylist(VideoPlaylistViewModel videoPlaylistViewModel);
        Task<IEnumerable<Playlist>> PlaylistList();
    }
}
