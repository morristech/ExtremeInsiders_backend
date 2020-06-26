﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExtremeInsiders.Areas.Api.Models;
using ExtremeInsiders.Data;
using ExtremeInsiders.Entities;
using ExtremeInsiders.Services;
using ExtremeInsiders.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExtremeInsiders.Areas.Api.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/[controller]/[action]")]
  public class UserController: Controller
  {
    private readonly ApplicationContext _db;
    private readonly UserService _userService;
    private readonly IEnumerable<SocialAuthService> _authServices;
    private readonly ConfirmationService _confirmationService;

    public UserController(ApplicationContext db, UserService userService, IEnumerable<SocialAuthService> authServices, ConfirmationService confirmationService)
    {
      _db = db;
      _userService = userService;
      _authServices = authServices;
      _confirmationService = confirmationService;
    }
    
    [HttpPut("{type}")]
    public async Task<IActionResult> SocialAccount(string type, AuthenticationModels.SocialLogIn model)
    {
      var handler = _authServices.FirstOrDefault(s => s.ProviderName == type);
      if (handler != null)
      {
        var user = _userService.User;
        var account = await handler.CreateAccount(model.Token);
        if (account != null)
        {
          user.SocialAccounts.Add(account);
          _db.SaveChanges();
          return Ok(user.WithoutSensitive(false));
        }

        ModelState.AddModelError("Auth", $"Не удалось привязать {type} аккаунт.");
        return BadRequest(ModelState);
      }

      ModelState.AddModelError("Auth", $"Неправильный тип социальной сети.");
      return NotFound(ModelState);
    }

    [HttpDelete("{type}")]
    public IActionResult SocialAccount(string type)
    {
      var user = _userService.User;
      var toRemove = user.SocialAccounts.SingleOrDefault(a => a.Provider.Name == type);

      if (toRemove != null)
      {
        user.SocialAccounts.Remove(toRemove);
        _db.SaveChanges();
        return Ok(user.WithoutSensitive(false));
      }

      ModelState.AddModelError("Auth", $"Неправильный тип социальной сети или аккаунт не найден.");
      return NotFound(ModelState);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Like(int id)
    {
      var like = await _db.Likes.FirstOrDefaultAsync(l => l.EntityId == id && l.UserId == _userService.UserId);
      if (like == null)
      {
        var entity = await _db.EntitiesLikeable.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        like = new Like
        {
          EntityId = entity.Id,
          UserId = _userService.UserId,
        };
        _db.Likes.Add(like);
      }
      else
      {
        _db.Remove(like);
      }

      _db.SaveChanges();
      return Ok();
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Favorite(int id)
    {
      var favorite = await _db.Favorites.FirstOrDefaultAsync(l => l.EntityId == id && l.UserId == _userService.UserId);
      if (favorite == null)
      {
        var entity = await _db.EntitiesBase.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) return NotFound();

        favorite = new Favorite
        {
          EntityId = entity.Id,
          UserId = _userService.UserId,
        };
        _db.Favorites.Add(favorite);
      }
      else
      {
        _db.Remove(favorite);
      }

      _db.SaveChanges();
      return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> VerifyEmail()
    {
      await _confirmationService.SendEmailConfirmationAsync(_userService.User);
      return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> VerifyEmail([FromBody] string code)
    {
      var confirmationCode = _userService.User.ConfirmationCodes.FirstOrDefault(x=>ConfirmationCode.CanBeUsed(x, code, ConfirmationCode.Types.EmailConfirmation));

      if (confirmationCode == null)
        return BadRequest();

      confirmationCode.IsConfirmed = true;
      await _db.SaveChangesAsync();

      return Ok();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] string email)
    {
      email = email.ToLower();
      var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
      if (user == null) return BadRequest();

      await _confirmationService.SendPasswordReset(user);

      return Ok();
    }

    [HttpPatch]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody]UserModels.PasswordReset model)
    {
      
      var confirmationCode = await _db.ConfirmationCodes.FirstOrDefaultAsync( ConfirmationCode.CanBeUsed(model.Code, ConfirmationCode.Types.PasswordReset));
      if (confirmationCode == null) return BadRequest();

      confirmationCode.IsConfirmed = true;
      
      var user = confirmationCode.User;
      user.Password = _userService.HashPassword(user, model.Password);
      await _db.SaveChangesAsync();
      
      return Ok();
    }
  }
}