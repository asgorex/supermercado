create dayabase supermercado;
use supermercado;

create table producto(
    id_producto int not null auto_increment primary key,
    nombre varchar(20),
    precio int
);