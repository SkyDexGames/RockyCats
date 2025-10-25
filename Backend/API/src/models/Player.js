const mongoose = require("mongoose");

const playerSchema = new mongoose.Schema({
  name: { type: String, required: true },
  levels: { type: [Number], default: [] },
  scores: { type: [Number], default: [] },
}, { timestamps: true });

module.exports = mongoose.model("Player", playerSchema);
