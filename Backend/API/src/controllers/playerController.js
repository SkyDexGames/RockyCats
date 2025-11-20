const Player = require("../models/Player");

exports.getPlayers = async (req, res) => {
  try {
    const players = await Player.find();
    res.json(players);
  } catch (err) {
    res.status(500).json({ message: "Error al obtener jugadores", error: err.message });
  }
};

exports.createPlayer = async (req, res) => {
  try {
    const player = new Player(req.body);
    await player.save();
    res.status(201).json({ message: "Jugador creado", player });
  } catch (err) {
    res.status(400).json({ message: "Error al crear jugador", error: err.message });
  }
};

exports.getPlayerByUsername = async (req, res) => {
  try {
    const player = await Player.findOne({ username: req.params.username });
    if (!player) {
      return res.status(404).json({ message: "Jugador no encontrado" });
    }
    res.json(player);
  } catch (err) {
    res.status(500).json({ message: "Error al obtener jugador", error: err.message });
  }
};


exports.updatePlayerLevels = async (req, res) => {
  try {
    const { username } = req.params;
    const { levels } = req.body;

    const player = await Player.findOne({ username });
    if (!player) {
      return res.status(404).json({ message: "Jugador no encontrado" });
    }

    player.levels = levels;
    await player.save();

    res.json({ message: "Niveles del jugador actualizados", player });
  } catch (err) {
    console.error("Error al actualizar niveles del jugador:", err.message);
    res.status(500).json({ message: "Error al actualizar niveles del jugador", error: err.message });
  }
};