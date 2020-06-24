﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ExtremeInsiders.Models;
using Newtonsoft.Json;

namespace ExtremeInsiders.Entities
{
  public class Playlist : EntityBase, ITranslatableEntity<Playlist, PlaylistTranslation>
  {
    [JsonIgnore]
    public int SportId { get; set; }
    public virtual Sport Sport { get; set; }
    
    public virtual List<Video> Videos { get; set; }

    [JsonIgnore]
    [ForeignKey("BaseEntityId")]
    public virtual List<PlaylistTranslation> Translations { get; set; }
    [NotMapped]
    public PlaylistTranslation Content { get; set; }
  }
  
  public class PlaylistTranslation : TranslatableEntityTranslation<Playlist>
  {
    public string Name { get; set; }
    public string Description { get; set; }
    
    [JsonIgnore]
    public int? ImageId { get; set; }
    public virtual Image Image { get; set; }
  }
}