ALTER TABLE helpdesk.Tbl_Case ADD MimeMessageCaseData nvarchar(MAX) NULL;

ALTER TABLE helpdesk.Tbl_Profile_CasosAsignados ADD Id_Tipo_Participacion int NULL;
ALTER TABLE helpdesk.Tbl_Profile_CasosAsignados ADD 
CONSTRAINT Tbl_Profile_CasosAsignados_Cat_Tipo_Participaciones_FK 
FOREIGN KEY (Id_Tipo_Participacion) REFERENCES helpdesk.Cat_Tipo_Participaciones(Id_Tipo_Participacion);

INSERT INTO helpdesk.Cat_Tipo_Participaciones
(Descripcion)
VALUES(N'Autor');

ALTER TABLE [security].Tbl_Profile ALTER COLUMN Nombres nvarchar(300) COLLATE Modern_Spanish_CI_AS NULL;
ALTER TABLE [security].Tbl_Profile ALTER COLUMN Apellidos nvarchar(300) COLLATE Modern_Spanish_CI_AS NULL;
ALTER TABLE [security].Tbl_Profile ALTER COLUMN Correo_institucional nvarchar(300) COLLATE Modern_Spanish_CI_AS NULL;

--v2024.09

CREATE TABLE PROYECT_MANAGER_BD.helpdesk.Tbl_Grupo (
	Id_Grupo int IDENTITY(1,1) NOT NULL,
	Descripcion nvarchar(150) COLLATE Latin1_General_CI_AI NULL,
	Estado nvarchar(50) COLLATE Latin1_General_CI_AI NULL,
	CONSTRAINT PK_Tbl_Grupo PRIMARY KEY (Id_Grupo)
);
ALTER TABLE PROYECT_MANAGER_BD.[security].Tbl_Profile ADD Id_Grupo int NULL;
ALTER TABLE PROYECT_MANAGER_BD.[security].Tbl_Profile ADD CONSTRAINT Tbl_Profile_FK FOREIGN KEY (Id_Grupo) REFERENCES PROYECT_MANAGER_BD.helpdesk.Tbl_Grupo(Id_Grupo);

ALTER TABLE PROYECT_MANAGER_BD.[security].Tbl_Profile ADD ORCID nvarchar(500) NULL;

