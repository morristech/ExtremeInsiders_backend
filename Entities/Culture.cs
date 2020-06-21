﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace ExtremeInsiders.Entities
{
  public class Culture
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public int Id { get; set; }
    public string Key { get; set; }
    
    public static Culture Russian = new Culture
    {
      Id = 1, Key = "ru"
    };

    public static Culture English = new Culture
    {
      Id = 2, Key = "en"
    };
    
    public static List<Culture> AllCultures = new List<Culture>
    {
      Russian,
      English
    }; 
  }

  // interface ITranslatable<T,TR> where T: class
  // {
  //   public List<TR> Translations { get; set; }
  //   public TR Content { get; set; }
  //   public T OfCulture(Culture culture);
  //   public T OfCulture(string culture);
  // }

  public static class TranslatableExtensions
  {
    // public static List<TranslatableEntity<T>> OfCulture<T>(this IEnumerable<TranslatableEntity<T>> sports, Culture culture) where T : ITranslatableEntityTranslation
    // {
    //   return sports.Select(s => s.OfCulture(culture)).ToList();
    // }
  }
}