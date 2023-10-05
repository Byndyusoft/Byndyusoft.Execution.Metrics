# Byndyusoft.Execution.Metrics

Набор инструментов для Open Telemetry,
который позволяет получать согласованные метрики и трассы для основных операций сервиса.

Под основной операцией понимается обработка http-запроса, выполнение периодического задания (hangfire), 
обработка сообщений очереди (rabbit-mq, kafka) и т.п. Т.е. то, что обычно является первым спаном в сервисе.

# Почему бы не взять стандартные библиотеки?

В соответствии с [семантическими соглашениями](https://opentelemetry.io/docs/specs/otel/metrics/semantic_conventions/) 
у разных основных операций метрики имеют разные названия, а трассы содержат разный набор тегов.

Из-за этого есть ряд проблем:
* Для каждой основной операции нужно своё правило алертинга, т.к. разные названия метрик и тегов.
* Для каждой основной операции нужно знать её теги, чтобы по ним искать трассы.
* Не для каждой операции есть готовое соглашение.

Если использовать одинаковый набор мертик и тегов для каждой операции, то эти проблемы уходят.

Появляется возможность построить универсальный дашборд для всех операций.
Настроить единые правила алертинга (хотя бы на старте проекта).
Искать по тегам связанные с метриками трассы.

Для этого нужно разработать свои собственные соглашения.

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
| Результат           | result            | result             | По умолчанию пусто. Заполняется, если нужно различать овтеты. Например: Retry, 200 и т.п. |

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


# Использование

Основа библиотеки - `ExecutionHandler`, который используется для простого добавления метрик и трасс для любой основной опрерации.
`ExecutionHandler` создаёт Activity, по которому вычисляются метрики и записываются трассы.

Чтобы заработала трассировка для операций, которые используют для метрик `ExecutionHandler`,
нужно добавить соответствующую инструментацию в трассировку:

```
.WithTracing(builder =>
    builder.AddExecutionDurationInstrumentation()
    ...
```

Чтобы заработали метрики для операций, которые используют `ExecutionHandler`,
нужно добавить соответствующую инструментацию в метрики:

```
.WithMetrics(builder =>
    builder.AddExecutionDurationInstrumentation()
    ...
```

Чтобы заработали метрики для http-запросов и не было дублирования со стандартными метриками,
нужно заменить `AddAspNetCoreInstrumentation` на `AddHttpRequestExecutionDurationInstrumentation`
```
.WithMetrics(builder =>
    builder.AddHttpRequestExecutionDurationInstrumentation()
    ...
```

Чтобы писались метрики `hangfire`, нужно добавить соответствующий фильтр 
```
services.AddHangfire((_, configuration) => 
    configuration.AddHangfireExecutionMetricDurationFilter()
    ...

```


Метрики для рэбита и прочих основных операций в разработке.