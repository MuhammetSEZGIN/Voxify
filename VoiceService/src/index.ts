import express from "express";
import cors from "cors";
import config from "./config";
import { LiveKitService } from "./services/LiveKitService";
import { createVoiceRouter } from "./routes/voiceRoutes";
import { errorHandler } from "./middlewares/errorHandler";

const app = express();

// ── Global Middleware ──
app.use(cors());

// Webhook endpoint'i için raw body gerekiyor (imza doğrulaması için).
// express.json() ile parse edilmeden önce raw text olarak yakalamalıyız.
app.use(
  "/api/voice/webhook",
  express.text({ type: "*/*" })
);

// Geri kalan endpoint'ler için standart JSON parser
app.use(express.json());

// ── Health Check ──
app.get("/health", (_req, res) => {
  res.json({ status: "ok", service: "voice-service" });
});

// ── Services (DI) ──
const liveKitService = new LiveKitService();

// ── Routes ──
app.use("/api/voice", createVoiceRouter(liveKitService));

// ── Error Handler (en sonda) ──
app.use(errorHandler);

// ── Start ──
app.listen(config.port, () => {
  console.log(`🎙️  Voice Service is running on port ${config.port}`);
  console.log(`   LiveKit URL: ${config.livekit.url}`);
});

export default app;
