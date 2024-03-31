import http from 'k6/http';

import { sleep } from 'k6';

export default function () {

http.setResponseCallback(http.expectedStatuses(200));
http.get('https://toshida-dotnet-buggyapp.azurewebsites.net/api/SlowRequest/sleep/5');

sleep(1);

}
