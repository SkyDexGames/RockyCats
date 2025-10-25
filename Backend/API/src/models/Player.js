const mongoose = require("mongoose");
const bcrypt = require("bcrypt");

const playerSchema = new mongoose.Schema({
  username: { type: String, required: true, unique: true },
  password: { type: String, required: true },
  email: { type: String, required: true, unique: true },
  levels: { type: Number, default: 0 },
}, { timestamps: true });


//Se encripta la contraseña antes de guardar el jugador
playerSchema.pre("save", async function (next) {
  if (!this.isModified("password")) return next();
  const salt = await bcrypt.genSalt(10);
  this.password = await bcrypt.hash(this.password, salt);
  next();
});


//Se compara la contraseña ingresada con la almacenada
playerSchema.methods.comparePassword = async function (password) {
  return bcrypt.compare(password, this.password);
};


module.exports = mongoose.model("Player", playerSchema);