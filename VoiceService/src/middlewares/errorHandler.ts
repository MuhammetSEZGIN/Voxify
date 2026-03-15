import { Request, Response, NextFunction } from "express";

/**
 * Global hata yakalama middleware'i.
 * Route handler'lardan next(err) ile iletilen hataları düzenli biçimde loglar
 * ve istemciye yapılandırılmış bir JSON yanıtı döner.
 */
export function errorHandler(
  err: Error,
  _req: Request,
  res: Response,
  _next: NextFunction
): void {
  console.error("Unhandled Error:", err.message);
  console.error(err.stack);

  const statusCode = (err as any).statusCode ?? 500;

  res.status(statusCode).json({
    error: err.message || "Internal Server Error",
  });
}
