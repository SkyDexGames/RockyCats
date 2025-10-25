# Guía completa: Instalar MongoDB y configurar RockycatsDB en Ubuntu

## 1. Actualizar el sistema

Abre una terminal y ejecuta:

```bash
sudo apt update
sudo apt upgrade -y
```

## 2. Instalar MongoDB

Instala la versión estable de MongoDB usando apt:

<code>
sudo apt install -y mongodb 
</code>


Verifica la versión instalada

<code> 
mongod --version
</code>  

## 3. Inicia Mongo

<code>
sudo systemctl start mongodb
</code>

Y verifica que haya iniciado de manera correcta

<code>
  sudo systemctl status mongodb
</code>

## 4 Entra a la consola de mongo

<code>
mongosh
</code>  

y despues crea la base de datos del juego

<code>
use RockycatsDB
</code>  

NOTA: la base de datos se creara al insertar el primer documento

