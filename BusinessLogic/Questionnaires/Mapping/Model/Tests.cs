using CAPA_DATOS;
using CAPA_DATOS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataBaseModel
{
	public class Tests : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? Id_test { get; set; }
		public string? Titulo { get; set; }
		public string? Descripcion { get; set; }
		public int? Grado { get; set; }
		public string? Tipo_test { get; set; }
		public string? Estado { get; set; }
		public int? Id_categoria { get; set; }
		public DateTime? Fecha_publicacion { get; set; }
		public DateTime? Created_at { get; set; }
		public DateTime? Updated_at { get; set; }
		public string? Referencias { get; set; }
		public int? Tiempo { get; set; }
		public int? Caducidad { get; set; }
		public string? Color { get; set; }
		public string? Image { get; set; }
		[ManyToOne(TableName = "Cat_Categorias_Test", KeyColumn = "Id_categoria", ForeignKeyColumn = "Id_categoria")]
		public Cat_Categorias_Test? Cat_Categorias_Test { get; set; }
		[OneToMany(TableName = "Pregunta_Tests", KeyColumn = "Id_test", ForeignKeyColumn = "Id_test")]
		public List<Pregunta_Tests>? Pregunta_Tests { get; set; }
		[OneToMany(TableName = "Resultados_Tests", KeyColumn = "Id_test", ForeignKeyColumn = "Id_test")]
		public List<Resultados_Tests>? Resultados_Tests { get; set; }

		public List<Tests> GetTests()
		{
			var tests = Get<Tests>();
			tests.ForEach(t =>
			{
				t.Pregunta_Tests?.ForEach(pt =>
				{
					if (pt.Cat_Tipo_Preguntas != null)
					{
						pt.Cat_Tipo_Preguntas.Cat_Valor_Preguntas = new Cat_Valor_Preguntas
						{ Id_tipo_pregunta = pt.Cat_Tipo_Preguntas.Id_tipo_pregunta }
						   .Get<Cat_Valor_Preguntas>();
					}

				});
			});
			return tests;
		}

		public object? SaveTets()
		{
			Image = FileService.setImage(Image);
			return Save();
		}
		public object? SaveResultado()
		{
			try
			{
				BeginGlobalTransaction();
				if (Resultados_Tests == null || Resultados_Tests.Count == 0)
				{
					return new ResponseService
					{
						status = 403,
						message = "No hay resultados procesados"
					};
				}
				Resultados_Tests?.ForEach(resultado =>
				{
					resultado.Save();
				});
				CommitGlobalTransaction();
				return new ResponseService
				{
					status = 200,
					message = "Test guardado correctamente"
				};
			}
			catch (System.Exception)
			{

				RollBackGlobalTransaction();
				return new ResponseService
				{
					status = 500,
					message = "Error al guardar test"
				};
			}

		}

		public object? UpdateTest()
		{
			Image = FileService.setImage(Image);
			return Update();
		}
	}
}
