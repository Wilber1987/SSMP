-- PROYECT_MANAGER_BD.helpdesk.Tbl_Grupo definition

-- Drop table

-- DROP TABLE PROYECT_MANAGER_BD.helpdesk.Tbl_Grupo;

CREATE TABLE PROYECT_MANAGER_BD.helpdesk.Tbl_Grupo (
	Id_Grupo int IDENTITY(1,1) NOT NULL,
	Id_Perfil_Crea int NULL,
	Id_Tipo_Grupo int NULL,
	Fecha_Creacion date NULL,
	Estado nvarchar(50) COLLATE Latin1_General_CI_AI NULL,
	Descripcion nvarchar(MAX) COLLATE Latin1_General_CI_AI NULL,
	Nombre nvarchar(500) COLLATE Latin1_General_CI_AI NULL,
	Color nvarchar(100) COLLATE Latin1_General_CI_AI NULL,
	CONSTRAINT PK_Tbl_Grupo PRIMARY KEY (Id_Grupo)
);


-- PROYECT_MANAGER_BD.helpdesk.Tbl_Grupos_Profile definition

-- Drop table

-- DROP TABLE PROYECT_MANAGER_BD.helpdesk.Tbl_Grupos_Profile;

CREATE TABLE PROYECT_MANAGER_BD.helpdesk.Tbl_Grupos_Profile (
	Id_Grupo int NOT NULL,
	Id_Perfil int NOT NULL,
	Fecha_Incorporacion date NULL,
	Estado nvarchar(50) COLLATE Latin1_General_CI_AI NULL,
	Id_TipoMiembro int NULL,
	CONSTRAINT PK_Tbl_Grupos_Profile PRIMARY KEY (Id_Grupo,Id_Perfil)
);

ALTER TABLE helpdesk.Tbl_Tareas ALTER COLUMN Titulo nvarchar(500) COLLATE Latin1_General_CI_AI NULL;
ALTER TABLE helpdesk.Tbl_Tareas ALTER COLUMN Descripcion nvarchar(MAX) COLLATE Latin1_General_CI_AI NULL;

INSERT INTO PROYECT_MANAGER_BD.[security].Security_Permissions
( Descripcion, Estado, Detalles)
VALUES( N'GESTOR_TAREAS', N'Activo', N'PUEDE GESTIONAR TAREAS CREARLAS EDITARLAS');

ALTER TABLE PROYECT_MANAGER_BD.helpdesk.Tbl_Grupo ALTER COLUMN Descripcion nvarchar(MAX) COLLATE Latin1_General_CI_AI NULL;
ALTER TABLE PROYECT_MANAGER_BD.helpdesk.Tbl_Grupo ALTER COLUMN Nombre nvarchar(500) COLLATE Latin1_General_CI_AI NULL;
ALTER TABLE PROYECT_MANAGER_BD.helpdesk.Tbl_Grupo ADD Color nvarchar(100) NULL;



EXEC PROYECT_MANAGER_BD.sys.sp_rename N'PROYECT_MANAGER_BD.helpdesk.Cat_Dependencias.[default]' , N'DefaultDependency', 'COLUMN';
