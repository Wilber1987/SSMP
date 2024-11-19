DROP SCHEMA questionnaires;

CREATE SCHEMA questionnaires;
-- questionnaires.Cat_Categorias_Test definition

-- Drop table

DROP TABLE questionnaires.Cat_Categorias_Test;
DROP TABLE questionnaires.Cat_Tipo_Preguntas;
DROP TABLE questionnaires.Cat_Valor_Preguntas;
DROP TABLE questionnaires.Tests;
DROP TABLE questionnaires.Pregunta_Tests;
DROP TABLE questionnaires.Resultados_Tests;
DROP TABLE questionnaires.Resultados_Pregunta_Tests;


CREATE TABLE questionnaires.Cat_Categorias_Test (
	Id_categoria int IDENTITY(1,1) NOT NULL,
	descripcion nvarchar(MAX) COLLATE Modern_Spanish_CI_AS NULL,
	imagen nvarchar(MAX) COLLATE Modern_Spanish_CI_AS NULL,
	estado nvarchar(50) COLLATE Modern_Spanish_CI_AS NULL,
	created_at datetime NULL,
	updated_at datetime NULL,
	CONSTRAINT PK__Cat_Cate__4A033A9332C9187A PRIMARY KEY (Id_categoria)
);

-- questionnaires.Cat_Tipo_Preguntas definition

-- Drop table
CREATE TABLE questionnaires.Cat_Tipo_Preguntas (
	id_tipo_pregunta int IDENTITY(1,1) NOT NULL,
	tipo_pregunta nvarchar(MAX) COLLATE Modern_Spanish_CI_AS NULL,
	descripcion nvarchar(MAX) COLLATE Modern_Spanish_CI_AS NULL,
	estado nvarchar(50) COLLATE Modern_Spanish_CI_AS NULL,
	fecha_crea datetime NULL,
	created_at datetime NULL,
	updated_at datetime NULL,
	editable bit NULL,
	descripcion_general nvarchar(MAX) COLLATE Modern_Spanish_CI_AS NULL,
	CONSTRAINT PK__Cat_Tipo__81F6D7D5551EB4EE PRIMARY KEY (id_tipo_pregunta)
);
-- questionnaires.Cat_Valor_Preguntas definition

-- Drop table
CREATE TABLE questionnaires.Cat_Valor_Preguntas (
	id_valor_pregunta int IDENTITY(1,1) NOT NULL,
	descripcion nvarchar(MAX) COLLATE Modern_Spanish_CI_AS NULL,
	id_tipo_pregunta int NULL,
	estado nvarchar(50) COLLATE Modern_Spanish_CI_AS NULL,
	valor int NULL,
	created_at datetime NULL,
	updated_at datetime NULL,
	CONSTRAINT PK__Cat_Valo__014E3C78AB16626B PRIMARY KEY (id_valor_pregunta),
	CONSTRAINT FK_Cat_Valor_Preguntas_Cat_Tipo_Preguntas FOREIGN KEY (id_tipo_pregunta) REFERENCES questionnaires.Cat_Tipo_Preguntas(id_tipo_pregunta)
);

-- questionnaires.Tests definition
-- Drop table

CREATE TABLE questionnaires.Tests (
	id_test int IDENTITY(1,1) NOT NULL,
	titulo nvarchar(MAX) COLLATE Modern_Spanish_CI_AS NULL,
	descripcion nvarchar(MAX) COLLATE Modern_Spanish_CI_AS NULL,
	grado int NULL,
	tipo_test nvarchar(100) COLLATE Modern_Spanish_CI_AS NULL,
	estado nvarchar(50) COLLATE Modern_Spanish_CI_AS NULL,
	id_categoria int NULL,
	fecha_publicacion datetime NULL,
	created_at datetime NULL,
	updated_at datetime NULL,
	referencias nvarchar(MAX) COLLATE Modern_Spanish_CI_AS NULL,
	tiempo int NULL,
	caducidad int NULL,
	color nvarchar(50) COLLATE Modern_Spanish_CI_AS NULL,
	[image] nvarchar(MAX) COLLATE Modern_Spanish_CI_AS NULL,
	CONSTRAINT PK__Tests__C6D3284B10E522BD PRIMARY KEY (id_test),
	CONSTRAINT FK_Tests_Cat_Categorias_Test FOREIGN KEY (id_categoria) REFERENCES questionnaires.Cat_Categorias_Test(Id_categoria)
);


-- questionnaires.Pregunta_Tests definition
-- Drop table

CREATE TABLE questionnaires.Pregunta_Tests (
	id_pregunta_test int IDENTITY(1,1) NOT NULL,
	estado nvarchar(50) COLLATE Modern_Spanish_CI_AS NULL,
	descripcion_pregunta nvarchar(MAX) COLLATE Modern_Spanish_CI_AS NULL,
	id_test int NULL,
	id_tipo_pregunta int NULL,
	fecha datetime NULL,
	created_at datetime NULL,
	updated_at datetime NULL,
	seccion nvarchar(MAX) COLLATE Modern_Spanish_CI_AS NULL,
	descripcion_general nvarchar(MAX) COLLATE Modern_Spanish_CI_AS NULL,
	CONSTRAINT PK__Pregunta__0F5B35929CDB72CF PRIMARY KEY (id_pregunta_test),
	CONSTRAINT FK_Pregunta_Tests_Cat_Tipo_Preguntas FOREIGN KEY (id_tipo_pregunta) REFERENCES questionnaires.Cat_Tipo_Preguntas(id_tipo_pregunta),
	CONSTRAINT FK_Pregunta_Tests_Tests FOREIGN KEY (id_test) REFERENCES questionnaires.Tests(id_test)
);
-- questionnaires.Resultados_Tests definition
-- Drop table

CREATE TABLE questionnaires.Resultados_Tests (
	Id_Perfil int NOT NULL,
	id_test int NOT NULL,
	fecha datetime NOT NULL,
	seccion nvarchar(MAX) COLLATE Modern_Spanish_CI_AS NULL,
	created_at datetime NULL,
	updated_at datetime NULL,
	valor nvarchar(MAX) COLLATE Modern_Spanish_CI_AS NULL,
	categoria_valor nvarchar(MAX) COLLATE Modern_Spanish_CI_AS NULL,
	tipo nvarchar(50) COLLATE Modern_Spanish_CI_AS NULL,
	Id_Resultado int IDENTITY(0,1) NOT NULL,
	CONSTRAINT Resultados_Tests_PK PRIMARY KEY (Id_Resultado),
	CONSTRAINT FK_Resultados_Tests_Tests FOREIGN KEY (id_test) REFERENCES questionnaires.Tests(id_test)
);


-- questionnaires.Resultados_Pregunta_Tests definition
-- Drop table



CREATE TABLE questionnaires.Resultados_Pregunta_Tests (
	Id_Perfil int NOT NULL,
	id_pregunta_test int NOT NULL,
	resultado int NULL,
	respuesta nvarchar(MAX) COLLATE Modern_Spanish_CI_AS NULL,
	estado nvarchar(50) COLLATE Modern_Spanish_CI_AS NULL,
	id_valor_pregunta int NULL,
	created_at datetime NULL,
	updated_at datetime NULL,
	fecha datetime NOT NULL,
	Id_Resultado int NULL,
	CONSTRAINT PK_Resultados_Pregunta_Tests PRIMARY KEY (Id_Perfil,id_pregunta_test,fecha),
	CONSTRAINT FK_Resultados_Pregunta_Tests_Cat_Valor_Preguntas FOREIGN KEY (id_valor_pregunta) REFERENCES questionnaires.Cat_Valor_Preguntas(id_valor_pregunta),
	CONSTRAINT FK_Resultados_Pregunta_Tests_Pregunta_Tests FOREIGN KEY (id_pregunta_test) REFERENCES questionnaires.Pregunta_Tests(id_pregunta_test),
	CONSTRAINT Resultados_Pregunta_Tests_FK FOREIGN KEY (Id_Resultado) REFERENCES questionnaires.Resultados_Tests(Id_Resultado)
);
