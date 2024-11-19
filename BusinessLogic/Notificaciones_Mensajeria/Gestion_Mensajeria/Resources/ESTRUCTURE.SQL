
SET QUOTED_IDENTIFIER ON
GO
DROP TABLE [dbo].[notificaciones]
GO
CREATE TABLE [dbo].[notificaciones]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Id_User] [int] NULL,
    [Titulo] [nvarchar](max) NULL,
    [Mensaje] [nvarchar](max) NULL,
    [Fecha] [datetime] NULL,
    [Media] [nvarchar](max) NULL,
    [Tipo] [nvarchar](50) NULL,
    [Estado] [nvarchar](50) NULL,
    [Enviado] [bit] NULL,
    [Leido] [bit] NULL,
    [Telefono] [nvarchar](20) NULL,
    [Email] [nvarchar](50) NULL,
    CONSTRAINT [PK_notificaciones] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


