# Что это?

Пакет `Byndyusoft.Execution.Metrics` - это набор инструментов для Open Telemetry,
который позволяет получать согласованные метрики и трассы для всех основных операций сервиса.

Под основной операцией понимается обработка http-запроса, выполнение периодического задания (hangfire), 
обработка сообщений очереди (rabbit-mq, kafka) и т.п.

# Зачем?

Если возникает проблемы с метрикой, появляется необходимость найти связанные с ней трассы (и логи).
Для этого в трассах должны быть все те же теги, что и в метриках.
Иначе придётся перебирать все трассы в нужном интервале времени.

Одинаковая метрика для всех основных операций позволяет использовать одно и то же правило для алертинга
и легко построить дашборд.

# Соглашения

## Требования к набору тегов

Для согласования метрик и трасс (и логов) в них должен быть один и тот же набор тегов:
* тип операции
* название операции
* код статуса
* результат. 

Это минимальный необходимый набор, чтобы работали дашборды и алертинг.
В метриках могут быть дополнительные теги, если этого требует проект. Например, тип документа.


| Тег                 | Название метриках | Название в трассах | Примечание |
|-|-|-|-|
| Тип операции        | type              | type               | hangfire, rabbit-mq, kafka и т.п. |
| Название операции   | operation         | Название спана     |  |
| Код статуса         | status_code       | otel.status_code   | "ERROR" или "OK", если результат не установлен, то тег не выводится |
| Результат           | result            | result             | "" или "200" или любой другой, если нужно различать ответы |

## Требования к метрикам

Метрика должна называться `execution_duration` и должна быть гистограммой.

Т.е. должны быть `execution_duration_ms_bucket` с тегом `le`, `execution_duration_ms_sum` и `execution_duration_ms_count`.

Пример: 
```
execution_duration_ms_bucket{operation="SendNotification",status_code="ERROR",status_description="",type="memory_queue",le="0"} 0 1695888400559
execution_duration_ms_bucket{operation="SendNotification",status_code="ERROR",status_description="",type="memory_queue",le="5"} 0 1695888400559
execution_duration_ms_bucket{operation="SendNotification",status_code="ERROR",status_description="",type="memory_queue",le="10"} 0 1695888400559
execution_duration_ms_bucket{operation="SendNotification",status_code="ERROR",status_description="",type="memory_queue",le="25"} 0 1695888400559
execution_duration_ms_bucket{operation="SendNotification",status_code="ERROR",status_description="",type="memory_queue",le="50"} 1 1695888400559
execution_duration_ms_bucket{operation="SendNotification",status_code="ERROR",status_description="",type="memory_queue",le="75"} 1 1695888400559
execution_duration_ms_bucket{operation="SendNotification",status_code="ERROR",status_description="",type="memory_queue",le="100"} 1 1695888400559
execution_duration_ms_bucket{operation="SendNotification",status_code="ERROR",status_description="",type="memory_queue",le="250"} 1 1695888400559
execution_duration_ms_bucket{operation="SendNotification",status_code="ERROR",status_description="",type="memory_queue",le="500"} 1 1695888400559
execution_duration_ms_bucket{operation="SendNotification",status_code="ERROR",status_description="",type="memory_queue",le="1000"} 1 1695888400559
execution_duration_ms_bucket{operation="SendNotification",status_code="ERROR",status_description="",type="memory_queue",le="+Inf"} 1 1695888400559
execution_duration_ms_sum{operation="SendNotification",status_code="ERROR",status_description="",type="memory_queue"} 48.0471 1695888400559
execution_duration_ms_count{operation="SendNotification",status_code="ERROR",status_description="",type="memory_queue"} 1 1695888400559
```

## Требования к именованию операций

Название операции формируется по разному в зависимости от её типа:
* **http** - "{GET|POST|etc} {url_template|url}". 
Т.к. собираем в метрики, урл не должен зависеть от входящих параметров, иначе сборщику метрик будет плохо.
* **hangfire** - "{Название операции из самого хенгфаера}"
* **rabbit_mq** - "{Название очереди на которую подписаны}"


## Как пользоваться?

Чтобы заработала трассировка для операций, которые используют `ExecutionHandler`

```
.AddOpenTelemetryTracing(builder =>
    builder.AddExecutionDurationInstrumentation()
    ...
```

Чтобы заработали метрики для операций, которые используют `ExecutionHandler`

```
.AddOpenTelemetryMetrics(builder =>
    builder.AddExecutionDurationInstrumentation()
    ...
```

Чтобы заработали метрики для http-запросов и не было дублирования, нужно заменить `AddAspNetCoreInstrumentation`
```
.AddOpenTelemetryMetrics(builder =>
    builder.AddHttpRequestExecutionDurationInstrumentation()
    ...
```

Чтобы писались метрики `hangfire`, нужно добавить фильтр 
```
services.AddHangfire((_, configuration) => 
    configuration.UseFilter(new HangfireExecutionMetricDurationFilter())
    ...

```


Метрики для рэбита и прочих основных операций в разработке.