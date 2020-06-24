﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ExtremeInsiders.Models;
using Newtonsoft.Json;

namespace ExtremeInsiders.Entities
{
  public class Sport : EntityBase, ITranslatableEntity<Sport, SportTranslation>
  {
    
    [JsonIgnore]
    public virtual List<Playlist> Playlists { get; set; }
    [JsonIgnore]
    public virtual List<Movie> Movies { get; set; }

    public List<int> PlaylistsIds => Playlists.Select(x => x.Id).ToList();
    public List<int> MoviesIds => Movies.Select(x => x.Id).ToList();

    [JsonIgnore]
    [ForeignKey("BaseEntityId")]
    public virtual List<SportTranslation> Translations { get; set; }
    [NotMapped]
    public SportTranslation Content { get; set; }
  }

  public class SportTranslation : TranslatableEntityTranslation<Sport>, IDefaultTranslatableContent
  {
    public string Name { get; set; }
    public string Description { get; set; }
    
    [JsonIgnore]
    public int? ImageId { get; set; }
    public virtual Image Image { get; set; }
  }
}