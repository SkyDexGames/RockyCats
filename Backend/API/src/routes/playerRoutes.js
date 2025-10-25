const express = require("express");
const router = express.Router();
const playerController = require("../controllers/playerController");
const auth = require("../middleware/auth"); // opcional, si quieres proteger las rutas

// Rutas públicas
router.get("/", playerController.getPlayers);
router.get("/:username", playerController.getPlayerByUsername);

// Si queremos proteger las rutas con autenticación
// router.get("/", auth, playerController.getPlayers);
//Lo super checamos pero puede ser


module.exports = router;
