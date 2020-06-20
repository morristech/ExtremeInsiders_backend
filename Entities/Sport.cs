﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace ExtremeInsiders.Entities
{
  public class Sport : ITranslatable<Sport, SportTranslation>
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [JsonIgnore]
    public virtual List<SportTranslation> Translations { get; set; }
    
    [JsonIgnore]
    public int? ImageId { get; set; }
    public virtual Image Image { get; set; }
    
    [NotMapped]
    public SportTranslation Content { get; set; }

    public Sport OfCulture(Culture culture)
    {
      Content = Translations.SingleOrDefault(tr => tr.Culture.Key == culture.Key);
      return this;
    }

    public Sport OfCulture(string culture)
    {
      Content = Translations.SingleOrDefault(tr => tr.Culture.Key == culture);
      return this;
    }
  }

  public class SportTranslation
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    
    [JsonIgnore]
    public int SportId { get; set; }
    public virtual Sport Sport { get; set; }
    
    [JsonIgnore]
    public int CultureId { get; set; }
    [JsonIgnore]
    public virtual Culture Culture { get; set; }
  }
  
}