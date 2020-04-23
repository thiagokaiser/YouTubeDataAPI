using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRepositoryPlaylist
    {
        Task<Playlist> PlaylistById(int id);
        Task<IEnumerable<Video>> PlaylistVideosById(int id);
        Task<IEnumerable<Playlist>> PlaylistList();
        Task VideoDel(int id);
        Task PlaylistDel(int id);
        Task PlaylistAdd(Playlist playlist);
        Task PlaylistUpdate(Playlist playlist);

    }
}
