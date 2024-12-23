using System;
using System.Collections.Generic;
using System.Linq;
using API.Controllers;
using BusinessLogic.SystemDocuments.Models;
using CAPA_DATOS;

namespace BusinessLogic.SystemDocuments.Operations
{
	public class ArticleOperation
	{       

		public static List<Article> GetAllArticles(Article article)
		{
			return article.Get<Article>();
		}

		public static ResponseService CreateArticle(Article article, string? Identity)
		{
			var user = AuthNetCore.User(Identity);
			article.Author = user.UserData?.Nombres;
			article.Id_User = user.UserData?.Id_User;
			article.Publish_Date = DateTime.Now;
			article.Status = true;
			article.Save();
			return new ResponseService()
			{
				message = "Articulo creado con exito",
				status = 200
			};
		}

		public static ResponseService UpdateArticle(Article article)
		{
			article.Update_Date = DateTime.Now;
			article.Update();			
			return article.Update();
		}

		public static ResponseService InactiveArticle(Article article)
		{
			new Article
			{
				Status = article.Status,
				Article_Id = article.Article_Id,
				Update_Date = article.Update_Date
			}.Update();
			return new ResponseService()
			{
				message = "Articulo inactivado con exito",
				status = 200
			};
		}
	}
}