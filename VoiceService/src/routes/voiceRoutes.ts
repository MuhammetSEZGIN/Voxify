import { Router } from "express";
import { VoiceController } from "../controllers/VoiceController";
import { WebhookController } from "../controllers/WebhookController";
import { ILiveKitService } from "../services/LiveKitService";

export function createVoiceRouter(liveKitService: ILiveKitService): Router {
  const router = Router();

  const voiceController = new VoiceController(liveKitService);
  const webhookController = new WebhookController();

  // Token alma endpoint'i
  router.get("/join-room/:roomId", voiceController.joinRoom);

  // LiveKit webhook endpoint'i
  router.post("/webhook", webhookController.handleWebhook);

  return router;
}
