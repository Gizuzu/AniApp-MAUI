using AniApp.Repositories.Shikimori;
using AniApp.Repositories.Shikimori.Models;

namespace AniApp.Pages;

public partial class MyAnimeList : ContentPage
{
	private List<ShikimoriAnimeInfo> _animeList = new List<ShikimoriAnimeInfo>();

	public MyAnimeList()
	{
		InitializeComponent();
	}

	private async void LoadAnimes()
	{
        _animeList = await ShikimoriRepository.search("");
    }

    public List<ShikimoriAnimeInfo> AnimeCollection
    {
        get { return _animeList; }
        set { _animeList = value; }
    }
}