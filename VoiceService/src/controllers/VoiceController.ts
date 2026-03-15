import { Request, Response, NextFunction } from "express";
import { ILiveKitService } from "../services/LiveKitService";

export class VoiceController {
  private readonly liveKitService: ILiveKitService;

  constructor(liveKitService: ILiveKitService) {
    this.liveKitService = liveKitService;
  }

  /**
   * GET /api/voice/join-room/:roomId
   *
   * Dönüş: { token: string }
   */
  joinRoom = async (
    req: Request,
    res: Response,
    next: NextFunction
  ): Promise<void> => {
    try {
      const roomId = req.params.roomId as string;
      const user = res.locals.user as { userId: string; userName: string } | undefined;

      if (!roomId) {
        res.status(400).json({ error: "roomId is required (route param)" });
        return;
      }

      if (!user) {
        res.status(401).json({ error: "Unauthorized user context" });
        return;
      }

      const token = await this.liveKitService.generateRoomToken(
        roomId,
        user.userId,
        user.userName
      );

      res.json({ token });
    } catch (err) {
      next(err);
    }
  };
}
