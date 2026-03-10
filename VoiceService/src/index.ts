import express from "express";
import cors from "cors";
import config from "./config";
import { LiveKitService } from "./services/LiveKitService";
import { createVoiceRouter } from "./routes/voiceRoutes";
import { errorHandler } from "./middlewares/errorHandler";
import { requestLogger } from "./middlewares/requestLogger";

const app = express();

// ── Global Middleware ──
app.use(cors({
  origin: (origin, callback) => {
    // Geliştirme aşamasında tüm originlere (IP'lere) izin ver
    // Canlıya çıkarken burayı kısıtlayabilirsin.
    callback(null, true);
  },
  credentials: true, // Headerlarda token vb. taşınması için şart
  methods: ["GET", "POST", "PUT", "DELETE", "OPTIONS"],
  allowedHeaders: ["Content-Type", "Authorization"]
}));app.use(requestLogger);

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
const PORT = config.port || 5044;
// "0.0.0.0" demek, bilgisayarın sahip olduğu tüm IP'lerden (localhost, 127.0.0.1, 192.168.x.x)
// gelen isteklere kapıyı aç demektir.
app.listen(PORT, "0.0.0.0", () => {
  console.log(`🎙️  Voice Service is running on ALL interfaces at port ${PORT}`);
  console.log(`   Local: http://localhost:${PORT}`);
  console.log(`   Network: http://192.168.5.122:${PORT}`); // Senin IP adresin
  console.log(`   LiveKit URL: ${config.livekit.url}`);
});

export default app;
