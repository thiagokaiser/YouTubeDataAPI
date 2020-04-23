using Core.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRepositoryYoutube
    {
        Task VideoUpdate(Core.Models.Video video);
        Task VideoAdd(Core.Models.Video video);
        Task<Core.Models.Video> VideoById(string id);
        Task<IEnumerable<Core.Models.Video>> VideoList();
        Task VideoDelAll();
        Task AddVideoToPlaylist(VideoPlaylistViewModel videoPlaylistViewModel);
    }
}
