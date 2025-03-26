using APPCORE;

namespace CAPA_NEGOCIO.MAPEO
{
    public class Tbl_VinculateCase : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? Id_Vinculate { get; set; }
		public string? Descripcion { get; set; }
		public DateTime? Fecha { get; set; }
		[OneToMany(TableName = "Tbl_Case", KeyColumn = "Id_Vinculate", ForeignKeyColumn = "Id_Vinculate")]
		public List<Tbl_Case>? Casos_Vinculados { get; set; }

		public object DesvincularCaso(Tbl_Case caseDV)
		{
			try
			{
				BeginGlobalTransaction();
				List<Tbl_Case> Tbl_Cases = new Tbl_Case()
				{ Id_Vinculate = caseDV.Id_Vinculate }.Get<Tbl_Case>();
				if (Tbl_Cases.Count == 2)
				{
					foreach (var item in Tbl_Cases)
					{
						if (item.Id_Case != caseDV.Id_Case)
						{
							desvicular(item);
						}
					}
					this.Id_Vinculate = caseDV.Id_Vinculate;
					Delete();
				}
				object? response = desvicular(caseDV);
				CommitGlobalTransaction();
				return response;
			}
			catch (System.Exception)
			{
				RollBackGlobalTransaction();
				throw;
			}
		}

		private static object? desvicular(Tbl_Case caseDV)
		{
			caseDV.Id_Vinculate = null;
			caseDV.Estado = Case_Estate.Activo.ToString();
			var response = caseDV.Update();
			return response;
		}

		public object? VincularCaso()
		{
			try
			{
				BeginGlobalTransaction();
				Fecha = DateTime.Now;
				Tbl_Case caseV = Casos_Vinculados.FirstOrDefault(c => c.Id_Vinculate != null);
				if (caseV != null)
				{
					Id_Vinculate = caseV.Id_Vinculate;
					List<Tbl_Case> oldCase = new Tbl_Case() { Id_Vinculate = caseV.Id_Vinculate }.Get<Tbl_Case>();
					foreach (var c in oldCase)
					{
						Tbl_Case caseSelected = Casos_Vinculados.FirstOrDefault(cv => cv.Id_Case == c.Id_Case);
						if (caseSelected == null)
						{
							Casos_Vinculados.Add(c);
						}
					}
					foreach (var c in Casos_Vinculados)
					{
						c.Id_Vinculate = Id_Vinculate;
					}

				}
				Descripcion = $"Vinculación de casos: {string.Join(", ", Casos_Vinculados.Select(c => "#" + c.Id_Case).ToList())}";
				int? caseFatherId = Casos_Vinculados.Where(x => x.Id_Case != null).ToList().Select(x => x.Id_Case).ToList().Min();
				foreach (var caseIten in Casos_Vinculados)
				{
					if (caseIten.Id_Case != caseFatherId)
					{
						caseIten.Estado = Case_Estate.Vinculado.ToString();
					}

				}
				if (caseV != null)
				{
					var response = Update();
					CommitGlobalTransaction();
					return response;
				}
				else
				{
					var response = Save();
					CommitGlobalTransaction();
					return response;
				}
			}
			catch (System.Exception)
			{
				RollBackGlobalTransaction();
				throw;
			}

		}
	}
}
