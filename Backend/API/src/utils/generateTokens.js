const jwt = require("jsonwebtoken");

function generateToken(player) {
  const { id, username } = player;
  return jwt.sign({ id, username }, process.env.JWT_SECRET, { expiresIn: "2h" });
}

module.exports = generateToken;
