using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ViewModels
{
    public class SearchResultViewModel
    {
        public string nextPageToken { get; set; }
        public string prevPageToken { get; set; }
        public IEnumerable<Video> Videos { get; set; }
        public IEnumerable<Playlist> Playlists { get; set; }
    }
}
