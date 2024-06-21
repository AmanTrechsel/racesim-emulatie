import pysoem
import time

def configure_and_run():
    try:
        # Initialize the EtherCAT master
        master = pysoem.Master()
        
        # Define the network interface (replace 'eth0' with your interface name)
        iface = 'eth0'
        master.open(iface)

        # Scan for slaves on the network
        if master.config_init() > 0:
            print(f'Found {len(master.slaves)} slaves')
            for i, slave in enumerate(master.slaves):
                print(f'Slave {i} name: {slave.name}')

            # Detailed initialization and configuration
            for slave in master.slaves:
                slave.reconfig()

            # Request pre-operational state for all slaves
            print("Setting state to SAFEOP_STATE...")
            master.state = pysoem.SAFEOP_STATE
            master.write_state()

            time.sleep(1)

            # Check state and errors
            state_check = master.state_check(pysoem.SAFEOP_STATE, 50000)
            if state_check != pysoem.SAFEOP_STATE:
                print(f'Error: Not all slaves reached safe-operational state. Current state: {state_check}')
                for i, slave in enumerate(master.slaves):
                    print(f'Slave {i} name: {slave.name}, state: {slave.state}, AL status code: {slave.al_status}')

            # Enter operational state
            print("Setting state to OP_STATE...")
            master.state = pysoem.OP_STATE
            master.write_state()
            
            time.sleep(1)

            # Check state and errors
            state_check = master.state_check(pysoem.OP_STATE, 50000)
            if state_check != pysoem.OP_STATE:
                print(f'Error: Not all slaves reached operational state. Current state: {state_check}')
                for i, slave in enumerate(master.slaves):
                    print(f'Slave {i} name: {slave.name}, state: {slave.state}, AL status code: {slave.al_status}')

            # Set the process data (turning the K30L2RGB7QP on or off)
            slave_index = 0  # Index of the EP6224-0002 slave (typically 0 if it's the only device)
            slot_index = 0  # Slot index for the K30L2RGB7QP

            def set_output(on):
                output_data = bytes(bytearray([0x01 if on else 0x00]))  # Convert bytearray to bytes
                print(f'Setting output to {"ON" if on else "OFF"}: {output_data}')
                master.slaves[slave_index].output = output_data

            # Turn on the indicator
            print("Turning on the indicator...")
            set_output(True)
            master.send_processdata()
            master.receive_processdata(2000)

            # Wait for a moment
            time.sleep(1)

            # Turn off the indicator
            print("Turning off the indicator...")
            set_output(False)
            master.send_processdata()
            master.receive_processdata(2000)

            # Return to safe-operational state
            print("Returning to SAFEOP_STATE...")
            master.state = pysoem.SAFEOP_STATE
            master.write_state()

        else:
            print('No slaves found')

    except Exception as e:
        print(f'Error: {e}')

    finally:
        master.close()

if __name__ == '__main__':
    configure_and_run()
