import { EMPTY, Observable } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';

export function handleEffect<T>(
  obs$: Observable<T>,
  onSuccess: (result: T) => void,
  onError?: (err: any) => void,
  onFinally?: () => void
): Observable<T> {
  return obs$.pipe(
    tap(onSuccess),
    catchError(err => {
      console.error(err);
      if (onError) onError(err);
      return EMPTY;
    }),
    finalize(() => {
      if (onFinally) onFinally();
    })
  );
}
