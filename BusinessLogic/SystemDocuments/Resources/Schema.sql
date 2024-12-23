create schema system_documents;
go 
drop table system_documents.category;
create table system_documents.category (
        category_id int primary key identity,
        Descripcion nvarchar(255)
);
go
drop table system_documents.article;
create table system_documents.article (
    article_id int primary key identity,
    title nvarchar(255),
    author nvarchar(255),
    body nvarchar(max),
    publish_date datetime,
    Id_User int,
    Update_Date datetime,
    Status bit,
    category_id int,
    foreign key (category_id) references system_documents.category(category_id)
);
