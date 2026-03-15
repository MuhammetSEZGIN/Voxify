import { Request, Response, NextFunction } from "express";
import { WebhookReceiver } from "livekit-server-sdk";
import config from "../config";

export class WebhookController {
  private readonly receiver: WebhookReceiver;

  constructor() {
    this.receiver = new WebhookReceiver(
      config.livekit.apiKey,
      config.livekit.apiSecret
    );
  }

  /**
   * POST /api/voice/webhook
   *
   * LiveKit sunucusundan gelen webhook olaylarını dinler.
   * İsteğin gerçekten LiveKit'ten geldiğini imza doğrulaması ile kontrol eder.
   *
   * İleride "5 dakika tek başına kalan kullanıcıyı odadan at" gibi
   * otomasyon kuralları bu olaylar üzerinden tetiklenecektir.
   */
  handleWebhook = async (
    req: Request,
    res: Response,
    next: NextFunction
  ): Promise<void> => {
    try {
      // LiveKit imza doğrulaması — raw body ve Authorization header gerektirir.
      const authHeader = req.get("Authorization");

      if (!authHeader) {
        res.status(401).json({ error: "Missing Authorization header" });
        return;
      }

      // req.body burada raw string (text/plain veya application/webhook+json)
      const body =
        typeof req.body === "string" ? req.body : JSON.stringify(req.body);

      const event = await this.receiver.receive(body, authHeader);

      // ── Loglama ──
      console.log("──── LiveKit Webhook Event ────");
      console.log(`  Event : ${event.event}`);

      if (event.participant) {
        console.log(`  Participant Identity : ${event.participant.identity}`);
        console.log(`  Participant Name     : ${event.participant.name}`);
        console.log(`  Participant SID      : ${event.participant.sid}`);
      }

      if (event.room) {
        console.log(`  Room Name : ${event.room.name}`);
        console.log(`  Room SID  : ${event.room.sid}`);
      }

      console.log("───────────────────────────────");

      // LiveKit 200 yanıt bekler
      res.status(200).json({ received: true });
    } catch (err) {
      console.error("Webhook validation failed:", err);
      res.status(401).json({ error: "Invalid webhook signature" });
    }
  };
}
