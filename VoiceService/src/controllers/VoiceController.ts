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
   * Query params:
   *   - userId   : Kullanıcı ID'si
   *   - userName : Kullanıcı adı
   *
   * Dönüş: { token: string }
   *
   * Not: İleride [Authorize] middleware eklendiğinde userId ve userName
   *      doğrudan JWT claim'lerinden alınacaktır.
   */
  joinRoom = async (
    req: Request,
    res: Response,
    next: NextFunction
  ): Promise<void> => {
    try {
      const roomId = req.params.roomId as string;
      const userId = req.query.userId as string | undefined;
      const userName = req.query.userName as string | undefined;

      if (!roomId) {
        res.status(400).json({ error: "roomId is required (route param)" });
        return;
      }

      if (!userId || !userName) {
        res
          .status(400)
          .json({ error: "userId and userName are required (query params)" });
        return;
      }

      const token = await this.liveKitService.generateRoomToken(
        roomId,
        userId,
        userName
      );

      res.json({ token });
    } catch (err) {
      next(err);
    }
  };
}
