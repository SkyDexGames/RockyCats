const generateToken = require("../utils/generateTokens");
const Player = require("../models/Player");

exports.register = async (req, res) => {
  try {
    const { username, email, password } = req.body;
    const existing = await Player.findOne({ email });
    if (existing) return res.status(400).json({ message: "Email ya registrado" });

    const player = new Player({ username, email, password });
    await player.save();

    res.status(201).json({ message: "Usuario registrado exitosamente" });
  } catch (err) {
    res.status(500).json({ message: "Error al registrar usuario", error: err.message });
  }
};

exports.login = async (req, res) => {
  try {
    const { email, password } = req.body;

    //Verificar si el usuario existe
    const player = await Player.findOne({ email });
    if (!player) return res.status(400).json({ message: "Usuario no encontrado" });
    //Verificar la contraseña
    const valid = await player.comparePassword(password);
    if (!valid) return res.status(400).json({ message: "Contraseña incorrecta" });

    //Se genera el token JWT
    const token = generateToken(player);

    res.status(200).json({
      message: "Login exitoso",
      token,
      user: { id: player._id, username: player.username, email: player.email },
    });
  } catch (err) {
    res.status(500).json({ message: "Error en login", error: err.message });
  }
};
