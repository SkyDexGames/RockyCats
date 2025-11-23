const express = require("express");
const helmet = require("helmet");
const cors = require("cors");
const playerRoutes = require("./routes/playerRoutes");
const authRoutes = require("./routes/authRoutes");
const connectDB = require("./config/db");

const app = express();

// Seguridad y middlewares
app.use(helmet());
app.use(cors());
app.use(express.json());

connectDB();

// Rutas principales
app.use("/api/players", playerRoutes);
app.use("/api/auth", authRoutes);

app.get("/", (req, res) => {
  res.json({ message: "API RockyAPI activa" });
});

module.exports = app;
