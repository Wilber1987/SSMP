using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using BusinessLogic.SystemDocuments.Models;
using BusinessLogic.SystemDocuments.Operations;
using CAPA_DATOS;
using CAPA_DATOS.Security;
using DataBaseModel;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class ApiArticleManagerController : ControllerBase
	{
		[HttpPost]
		public List<Article> GetArticle(Article article)
		{
			return ArticleOperation.GetAllArticles(article);
		}

		[HttpPost]
		[AuthController(Permissions.MANAGE_ARTICLES)]
		public ResponseService SaveArticle(Article article)
		{
			return ArticleOperation.CreateArticle(article, HttpContext.Session.GetString("seassonKey"));
		}

		[HttpPost]
		[AuthController(Permissions.MANAGE_ARTICLES)]
		public ResponseService UpdateArticle(Article article)
		{
			return ArticleOperation.UpdateArticle(article);
		}

		[HttpPost]
		[AuthController(Permissions.MANAGE_ARTICLES)]
		public ResponseService InactiveArticle(Article article)
		{
			return ArticleOperation.InactiveArticle(article);
		}
		
		[HttpPost]
		public List<Category> GetCategory(Category inst)
		{
			return inst.Get<Category>();
		}

		[HttpPost]
		[AuthController(Permissions.MANAGE_ARTICLES)]
		public object? SaveCategory(Category inst)
		{
			return inst.Save();
		}

		[HttpPost]
		[AuthController(Permissions.MANAGE_ARTICLES)]
		public ResponseService UpdateCategory(Category inst)
		{
			return  inst.Update();
		}
	}
}
