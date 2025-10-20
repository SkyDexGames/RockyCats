const express = require("express");
const router = express.Router();
const authController = require("../controllers/authController");
const auth = require("../middleware/auth"); // opcional, si quieres proteger las rutas

router.post("/RegisterPlayer/", authController.register);
router.post("/LoginPlayer/", authController.login);

module.exports = router;